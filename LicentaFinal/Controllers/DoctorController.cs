using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.Repositories;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;
using NToastNotify.Helpers;

namespace LicWeb.Controllers
{
    [Authorize(Roles = "doctor")]
    
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment Environment;
        private readonly IAdeverintaRepository _adeverintaRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ICheieRepository _cheieRepository;
        public DoctorController(IWebHostEnvironment environment
            ,IAdeverintaRepository adeverintaRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            IDoctorRepository doctorRepository, 
            ApplicationDbContext context,
            ICheieRepository cheieRepository)
        {
            Environment = environment;
            _adeverintaRepository = adeverintaRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _doctorRepository = doctorRepository;
            _context = context;
            _cheieRepository = cheieRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult TrimiteAdeverinta()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TrimiteAdeverinta(AdeverintaViewModel adeverintaViewModel)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var PKEY = adeverintaViewModel.CheiePrivata;
                List<string> uploadedPKEY = new List<string>();
                string PKEYFileName = $@"{Guid.NewGuid()}.pem"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, PKEYFileName), FileMode.Create))
                {
                    PKEY.CopyTo(stream);
                    uploadedPKEY.Add(PKEYFileName);
                    ViewBag.Message += PKEYFileName + ",";
                    stream.Close();
                }
                var adeverinta = adeverintaViewModel.Adeverinta;
                List<string> uploadedAdeverinta = new List<string>();
                string adeverintaFileName = $@"{Guid.NewGuid()}.pdf"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, adeverintaFileName), FileMode.Create))
                {
                    adeverinta.CopyTo(stream);
                    uploadedAdeverinta.Add(adeverintaFileName);
                    ViewBag.Message += adeverintaFileName + ",";
                    stream.Close();
                }
                CertificateAuthority CA = new CertificateAuthority();
                string signature = CA.SignData(adeverintaFileName, PKEYFileName);

                await _userManager.GetUserAsync(HttpContext.User);
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var DocId = _doctorRepository.GetByUID(userId);
                var IdCheiePublica = DocId.IdCheiePublica;
                var cheie = await _cheieRepository.GetByIdAsync(IdCheiePublica);
                int passed;
                var getStudentId = _userRepository.GetIdByEmail(adeverintaViewModel.EmailStudent);
                if (CA.VerifySignature(cheie.CheiePublica, signature, adeverintaFileName))
                {
                    passed = 1;
                    Debug.WriteLine("Passed:" + cheie.CheiePublica + "\n" + DocId.Id + "\n" + IdCheiePublica);
                }
                else
                {
                    passed = 0;
                    Debug.WriteLine("NOT Passed:" + cheie.CheiePublica + "\n" + DocId.Id + "\n" + IdCheiePublica);
                }

                var adeverintaToDb = new Adeverinta()
                {
                    IdDoctor = DocId.Id,
                    IdStudent = getStudentId,
                    SemnaturaDoctor = signature,
                    CurrentStatus = passed,
                    StartDate = adeverintaViewModel.MotivareDin,
                    EndDate = adeverintaViewModel.MotivarePana,
                    PathToAdeverinta = adeverintaFileName,
                    DataConsultatie = adeverintaViewModel.DataConsultatie,
                    SemnaturaUniversitate = "not signed yet"
                };
                var adeverintaResponse = _adeverintaRepository.Add(adeverintaToDb);
                if (adeverintaResponse)
                {
                    _adeverintaRepository.Save();
                    TempData["Succes"] = "Adeverinta a fost trimisa";
                    return View(adeverintaViewModel);
                }
                else
                {
                    TempData["Eroare"] = "A intervenit o eroare";
                    return View(adeverintaViewModel);
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult GenereazaAdeverinta()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenereazaAdeverinta(GenerareAdeverinta adeverintaViewModel)
        {
            var output = new MemoryStream();
            var document = new Document();

            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;

            document.Open();

            var paragraph = new Paragraph();
            var fontSize = 18f; 
            var font = FontFactory.GetFont(FontFactory.TIMES, fontSize);
            var fontModel = FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, fontSize);
            paragraph.Leading = 40f;
            var titleParagraph = new Paragraph("ADEVERINTA MEDICALA", FontFactory.GetFont(FontFactory.TIMES_BOLD, 32f));
            titleParagraph.Alignment = Element.ALIGN_CENTER;
            titleParagraph.SpacingAfter = 20f;

            document.Add(titleParagraph);
            paragraph.Add(new Chunk($"Se adevereste ca ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.NumeStudent}", fontModel));
            paragraph.Add(new Chunk($"Sexul ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.Sex}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Nascut: ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.DataNasterii.ToString("dd.MM.yyyy")}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Cu domiciliul in ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.Domiciliu}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Avand ocupatia de ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.Ocupatie}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Este suferind de ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.Diagnostic}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Se recomanda ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.Recomandare}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"S-a eliberat prezenta spre a-i servi la: ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.MotivEliberare}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Data eliberarii: ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.DataEliberare.ToString("dd.MM.yyyy")}", fontModel));
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"CNP: ", font));
            paragraph.Add(new Chunk($"{adeverintaViewModel.CNP}", fontModel));
            
            string doctorID = _userManager.GetUserId(User);
            var doctorUser = await _userRepository.GetByIdAsync(doctorID);
            paragraph.Add(Chunk.NEWLINE);
            paragraph.Add(new Chunk($"Adeverinta a fost generata in cadrul solutiei de catre Dr. {doctorUser.UserName} la data de {DateTime.Now}.", font));
            paragraph.Add(Chunk.NEWLINE);
            var doctor = _doctorRepository.GetByUID(doctorID);
            paragraph.Add(new Chunk($"Parafa: {doctor.Parafa}", font));
            document.Add(paragraph);
            document.Close();
            output.Position = 0;

            return File(output, "application/pdf", "Adeverinta.pdf");
        }
    }
}
