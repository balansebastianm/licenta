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

        public string GenerateCertRequest(AsymmetricCipherKeyPair keyPair, string Email, string Localitate, string Judet)
        {
            //valori CSR
            string x509 = "CN=" + Email + ", O=Doctori Familie, L=" + Localitate + ", ST=" + Judet + ". C=RO";
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
            string FileToWriteTo = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\" + Email + ".csr";
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
                string PublicPEMFile = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\public-key-" + Email +".pem";
                string PrivatePEMFile = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\private-key-" + Email + ".pem";
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

                var transportKey = RemovePemHeaderFooter(keyPem.ToString());
                var csrData = GenerateCertRequest(keyPair, Email, Localitate, Judet);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void GenerateCertFromCSR(string Email)
        {
            string CSRPath = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\" + Email + ".csr";
            char[] caractere = File.ReadAllText(CSRPath).ToCharArray();

            //citim request-ul;
            byte[] csrEncode = Convert.FromBase64CharArray(caractere, 0, caractere.Length);
            Pkcs10CertificationRequest pk10holder = new Pkcs10CertificationRequest(csrEncode);
            string SubjectData = (pk10holder.GetCertificationRequestInfo().Subject).ToString();
            //verificam validitatea csr-ului
            bool verify = pk10holder.Verify();
            if (verify == false)
            {
                return;
            }
            //rng
            CryptoApiRandomGenerator randomGenerator = new();
            var random = new SecureRandom(randomGenerator);
            //importam certificatul de la autoritate
            ISignatureFactory signatureFactory;

            AsymmetricCipherKeyPair PerecheChei = null;
            TextReader ReadPrivatePEM = File.OpenText("C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Root Cert\\RootCert-private.pem");
            PerecheChei = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(ReadPrivatePEM).ReadObject();
            AsymmetricKeyParameter privateKey = PerecheChei.Private;
            byte[] issuer = File.ReadAllBytes("C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Root Cert\\RootCert.der");
            //deschidem certificatul issuer
            var issuerCertificate = new Org.BouncyCastle.X509.X509Certificate(issuer);
            var authorityKeyIdentifier = new AuthorityKeyIdentifierStructure(issuerCertificate);
            signatureFactory = new Asn1SignatureFactory(
            PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
            privateKey);
            //cream certificatul
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            //cream serial number pt certificat
            Org.BouncyCastle.Math.BigInteger serialNumber = BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            //setam issuer
            var issuerDN = new X509Name("CN=Autoritate, OU=Administrare, O=Universitate, L=Tg. Mures, C=RO");
            certificateGenerator.SetIssuerDN(issuerDN);
            //setam subject
            var subjectDN = new X509Name(SubjectData);
            certificateGenerator.SetSubjectDN(subjectDN);
            //expira intr-un an
            certificateGenerator.SetNotAfter(DateTime.UtcNow.AddMonths(12));
            certificateGenerator.SetNotBefore(DateTime.UtcNow);
            certificateGenerator.SetPublicKey(pk10holder.GetPublicKey());
            //legam certificatul de autoritate
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifier);
            Org.BouncyCastle.X509.X509Certificate cert = certificateGenerator.Generate(signatureFactory);
            //scriem certificatul
            string PathToCert = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\certificat-" + Email + ".der";
            using (var TextWriter = File.OpenWrite(PathToCert))
            {
                var buffer = cert.GetEncoded();
                TextWriter.Write(buffer, 0, buffer.Length);
                TextWriter.Close();
            }
            //adaugam certificatul in store(bypass warning)
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
            string PK = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\" + "public-key-" + Email + ".pem";
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

            AsymmetricCipherKeyPair PerecheChei = null;
            TextReader ReadPrivatePEM = File.OpenText(("C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\wwwroot\\uploads\\" + PathToPrivateKey));
            PerecheChei = (AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(ReadPrivatePEM).ReadObject();
            AsymmetricKeyParameter privateKey = PerecheChei.Private;
            ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");
            signer.Init(true, privateKey);
            byte[] bytes = File.ReadAllBytes("C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\wwwroot\\uploads\\" + DataToSign);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            byte[] signature = signer.GenerateSignature();
            var signedString = Convert.ToBase64String(signature);
            return signedString;

        }
        public bool VerifySignature(string PublicKey, string Signature, string pathToAdeverinta)
        {
            byte[] BytesToSign = File.ReadAllBytes("C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\wwwroot\\uploads\\" + pathToAdeverinta);
            byte[] ExpectedSignatureBytes = Convert.FromBase64String(Signature);
            string adaptedPK = "-----BEGIN PUBLIC KEY-----" + PublicKey + "-----END PUBLIC KEY-----";
            StringReader publicKeyReader = new StringReader(adaptedPK);
            Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(publicKeyReader);
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pemReader.ReadObject();

            ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");
            signer.Init(false, publicKey);
            signer.BlockUpdate(BytesToSign, 0, BytesToSign.Length);
            return signer.VerifySignature(ExpectedSignatureBytes);

        }
    }
}
