using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.X509;
using System.Diagnostics;
using Org.BouncyCastle.Utilities.IO.Pem;
using System.Security.Cryptography.Pkcs;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
namespace LicWeb
{
    public class CertificateAuthority
    {
        static string RemovePemHeaderFooter(string input)
        {
            var headerFooterList = new List<string>()
                    {
                        "-----BEGIN CERTIFICATE REQUEST-----",
                        "-----END CERTIFICATE REQUEST-----",
                        "-----BEGIN PUBLIC KEY-----",
                        "-----END PUBLIC KEY-----",
                        "-----BEGIN RSA PRIVATE KEY-----",
                        "-----END RSA PRIVATE KEY-----"
                    };

            string trimmed = input;
            foreach (var hf in headerFooterList)
            {
                trimmed = trimmed.Replace(hf, string.Empty);
            }

            return trimmed.Replace("\r\n", string.Empty);
        }
        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            // Generate private/public key pair
            RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
            KeyGenerationParameters keyParams = new KeyGenerationParameters(new SecureRandom(), 2048);
            generator.Init(keyParams);
            return generator.GenerateKeyPair();
        }

        [Obsolete]
        public string GenerateCertRequest(AsymmetricCipherKeyPair keyPair, string Email, string Localitate, string Judet)
        {
            //valori CSR
            string x509 = "CN=" + Email + ", O=Medici, L=" + Localitate + ", ST=" + Judet + ". C=RO";
            var subject = new X509Name(x509);
            var csr = new Pkcs10CertificationRequest(
            new Asn1SignatureFactory("SHA256withRSA", keyPair.Private),
            subject,
            keyPair.Public,
            null,
            keyPair.Private);
                              //scriem CSR-ul
            var csrPem = new StringBuilder();
            var csrPemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(new StringWriter(csrPem));
            csrPemWriter.WriteObject(csr);
            csrPemWriter.Writer.Flush();
            string FileToWriteTo = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\" + Email + ".csr";
            using (TextWriter textWriter = new StreamWriter(FileToWriteTo, false))
            {
                textWriter.Write(RemovePemHeaderFooter(csrPem.ToString()));
                textWriter.Flush();
                textWriter.Close();
            }
            return RemovePemHeaderFooter(csrPem.ToString());
        }
        public void GenerateCSR(string Email, string Localitate, string Judet)

        {
            try
            {
                var keyPair = GenerateKeyPair();
                var keyPem = new StringBuilder();
                var keyPemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(new StringWriter(keyPem));
                string PublicPEMFile = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\public-key-" + Email +".pem";
                string PrivatePEMFile = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\private-key-" + Email + ".pem";
                //scriem cheia publica
                using (TextWriter textWriter = new StreamWriter(PublicPEMFile, false))
                {
                    Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                    pemWriter.WriteObject(keyPair.Public);
                    pemWriter.Writer.Flush();
                    textWriter.Close();
                }

                //scriem cheia privata
                using (TextWriter textWriter = new StreamWriter(PrivatePEMFile, false))
                {
                    Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                    pemWriter.WriteObject(keyPair.Private);
                    pemWriter.Writer.Flush();
                    textWriter.Close();
                }

                keyPemWriter.WriteObject(keyPair.Public);
                keyPemWriter.Writer.Flush();
                var csrData = GenerateCertRequest(keyPair, Email, Localitate, Judet);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void GenerateCertFromCSR(string Email)
        {
            string CSRPath = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\" + Email + ".csr";
            char[] caractere = File.ReadAllText(CSRPath).ToCharArray();
            byte[] csrEncode = Convert.FromBase64CharArray(caractere, 0, caractere.Length);
            Pkcs10CertificationRequest pk10holder = new Pkcs10CertificationRequest(csrEncode);
            string SubjectData = (pk10holder.GetCertificationRequestInfo().Subject).ToString();
            bool verify = pk10holder.Verify();
            if (verify == false)
            {
                return;
            }
            CryptoApiRandomGenerator randomGenerator = new();
            var random = new SecureRandom(randomGenerator);
            ISignatureFactory signatureFactory;
            AsymmetricCipherKeyPair? PerecheChei = null;
            TextReader ReadPrivatePEM = File.OpenText("C:\\licenta\\LicentaFinal\\RootCert\\RootCert-private.pem");
            PerecheChei = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(ReadPrivatePEM).ReadObject();
            AsymmetricKeyParameter privateKey = PerecheChei.Private;
            byte[] issuer = File.ReadAllBytes("C:\\licenta\\LicentaFinal\\RootCert\\RootCert.der");
            var issuerCertificate = new Org.BouncyCastle.X509.X509Certificate(issuer);
            var authorityKeyIdentifier = new AuthorityKeyIdentifierStructure(issuerCertificate);
            signatureFactory = new Asn1SignatureFactory(
            PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
            privateKey);
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            Org.BouncyCastle.Math.BigInteger serialNumber = BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            var issuerDN = new X509Name("CN=Autoritate, OU=Administrare, O=Universitate, L=Tg. Mures, C=RO");
            certificateGenerator.SetIssuerDN(issuerDN);
            var subjectDN = new X509Name(SubjectData);
            certificateGenerator.SetSubjectDN(subjectDN);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.AddMonths(12));
            certificateGenerator.SetNotBefore(DateTime.UtcNow);
            certificateGenerator.SetPublicKey(pk10holder.GetPublicKey());
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifier);
            Org.BouncyCastle.X509.X509Certificate cert = certificateGenerator.Generate(signatureFactory);
            string PathToCert = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\certificat-" + Email + ".der";
            using (var TextWriter = File.OpenWrite(PathToCert))
            {
                var buffer = cert.GetEncoded();
                TextWriter.Write(buffer, 0, buffer.Length);
                TextWriter.Close();
            }
            using(X509Store store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert.GetEncoded());
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();
            }
        }
        public string GetPkeyRegister(string Email)
        {
            //deschidem cheia publica a utilizatorului
            string PK = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\" + "public-key-" + Email + ".pem";
            Org.BouncyCastle.OpenSsl.PemReader pemReader;
            using (StreamReader reader = new StreamReader(PK))
            {
                 pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object pemObject = pemReader.ReadObject();
                if (pemObject is AsymmetricKeyParameter keyParam)
                {
                    //citim cheia publica
                    StringWriter stringWriter = new StringWriter();
                    Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(stringWriter);
                    pemWriter.WriteObject(keyParam);
                    pemWriter.Writer.Flush();

                    //normalizam cheia publica inainte de returnare (remove header/footer)
                    string PublicKeyNormalized = RemovePemHeaderFooter(stringWriter.ToString());
                    stringWriter.Close();
                    reader.Close();
                    return PublicKeyNormalized;
                }
                else
                {
                    throw new InvalidDataException("The file does not contain a valid public key in PEM format.");
                }
            }
        }
        public string SignData(string DataToSign, string PathToPrivateKey)
        {

            AsymmetricCipherKeyPair? PerecheChei = null;
            TextReader ReadPrivatePEM = File.OpenText(("C:\\licenta\\LicentaFinal\\wwwroot\\uploads\\" + PathToPrivateKey));
            PerecheChei = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(ReadPrivatePEM).ReadObject();
            AsymmetricKeyParameter privateKey = PerecheChei.Private;
            ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, privateKey);
            byte[] bytes = File.ReadAllBytes("C:\\licenta\\LicentaFinal\\wwwroot\\uploads\\" + DataToSign);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            byte[] signature = signer.GenerateSignature();
            var signedString = Convert.ToBase64String(signature);
            return signedString;

        }
        public string SignData(string DataToSign, string PathToPrivateKey, int a)
        {
            if(a != 1)
            {
                return "0";
            }
            AsymmetricCipherKeyPair? PerecheChei = null;
            TextReader ReadPrivatePEM = File.OpenText(("C:\\licenta\\LicentaFinal\\RootCert\\" + PathToPrivateKey));
            PerecheChei = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(ReadPrivatePEM).ReadObject();
            AsymmetricKeyParameter privateKey = PerecheChei.Private;
            ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");
            signer.Init(true, privateKey);
            byte[] bytes = File.ReadAllBytes("C:\\licenta\\LicentaFinal\\wwwroot\\uploads\\" + DataToSign);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            byte[] signature = signer.GenerateSignature();
            var signedString = Convert.ToBase64String(signature);
            return signedString;

        }
        public bool VerifySignature(string PublicKey, string Signature, string pathToAdeverinta)
        {
            byte[] BytesToSign = File.ReadAllBytes("C:\\licenta\\LicentaFinal\\wwwroot\\uploads\\" + pathToAdeverinta);
            byte[] ExpectedSignatureBytes = Convert.FromBase64String(Signature);
            string adaptedPK = "-----BEGIN PUBLIC KEY-----" + PublicKey + "-----END PUBLIC KEY-----";
            StringReader publicKeyReader = new StringReader(adaptedPK);
            Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(publicKeyReader);
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pemReader.ReadObject();

            ISigner signer = SignerUtilities.GetSigner("SHA256WithRSA");
            signer.Init(false, publicKey);
            signer.BlockUpdate(BytesToSign, 0, BytesToSign.Length);
            return signer.VerifySignature(ExpectedSignatureBytes);
        }
        public void EncryptFile(string pw, string PKPrivatePem, string email)
        {
            string sourceFilePath = PKPrivatePem;
            string archiveFilePath = @"C:\licenta\LicentaFinal\Certificate-Requests\" + email + ".zip";
            string password = pw;
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile())
            {
                zipFile.Password = password;
                zipFile.AddFile(sourceFilePath, "");
                zipFile.Save(archiveFilePath);
                }
                System.IO.File.Delete(PKPrivatePem);
        }
    }
}
