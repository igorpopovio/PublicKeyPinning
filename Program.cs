using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PublicKeyPinning {
    public class Program {
        private const string Uri = "https://gist.githubusercontent.com/igorpopovio/67e6d90c487394a99a87b41391b139dd/raw/a65886bb470c2a260469ae9572d1ff75184e4f91/does-it-work";

        private const string PinnedPublicKey = "3082010A0282010100C6D3F18A3BCFA445F2CB7067D7459FA1698A4D6EF9DD4BF63EEB033666A5C7FEE6A85AA2E41A8AE315901D0812A7285E760B562175822461ED80555C93E0C101B1E21EC13AEDEC295756B69761A9A8D0854D4EFB52CA0D543FF13F2C7793E70F5FDCBCAEA8CC899077C6CD7328360191CA0156B03E88EDF6DD89099822C45C23B63BB6F5B702C55A437031DEDEEE7B5EBB6B8232FC4DA79420DB63089F7DEDD9E80C3DF20353F4DC2837F26ADCB9FACE85DE0CE1EDE2209EA3503744FFE5FA5A624A9DC7C8F6D500EC23217F09F4A9039A8A2EE865BAEF31AD46E7734322817ED54E14BD3DB7F131243571041F6C6771A103494CD1F15EFF994D70312828EEE70203010001";

        public static void Main(string[] args) {
            ServicePointManager.ServerCertificateValidationCallback = PinPublicKey;

            Console.WriteLine(GetPageAt(Uri));

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        private static bool PinPublicKey(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            if (certificate == null || chain == null) return false;
            if (sslPolicyErrors != SslPolicyErrors.None) return false;

            // if the keys do not match then the connection will fail with:
            // System.Net.WebException: The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
            // AuthenticationException: The remote certificate is invalid according to the validation procedure.
            return certificate.GetPublicKeyString() == PinnedPublicKey;
        }

        private static string GetPageAt(string uri) {
            var request = WebRequest.Create(uri);
            var response = request.GetResponse(); // this will fail if the keys do not match

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return reader.ReadToEnd();
        }
    }
}
