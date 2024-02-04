namespace WingetNexus.WingetApi.Data
{
    public class ApiConstants
    {
        /// <summary>
        /// CertificateAuthenticationRequiredName environmental variable name.
        /// </summary>
        public const string CertificateAuthenticationRequiredEnvName = "CertificateAuthenticationRequired";

        /// <summary>
        /// CertificateAuthenticationSelfSigned environmental variable name.
        /// </summary>
        public const string CertificateAuthenticationSelfSignedEnvName = "CertificateAuthenticationSelfSigned";

        /// <summary>
        /// CertificateAuthenticationSubjectName environmental variable name.
        /// </summary>
        public const string CertificateAuthenticationSubjectNameEnvName = "CertificateAuthenticationSubjectName";
        /// <summary>
        /// Gets whether certificate authentication is enabled.
        /// </summary>
        public static string CertificateAuthenticationRequiredEnv
        {
            get
            {
                var result = Environment.GetEnvironmentVariable(CertificateAuthenticationRequiredEnvName);
                if (string.IsNullOrEmpty(result))
                {
                    return "false";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets whether self signed certificates are allowed.
        /// </summary>
        public static string CertificateAuthenticationSelfSignedEnv
        {
            get
            {
                var result = Environment.GetEnvironmentVariable(CertificateAuthenticationSelfSignedEnvName);
                if (string.IsNullOrEmpty(result))
                {
                    return "";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the expected root/intermediate cert subject name.
        /// </summary>
        public static string CertificateAuthenticationSubjectNameEnv
        {
            get
            {
                var result = Environment.GetEnvironmentVariable(CertificateAuthenticationSubjectNameEnvName);
                if (string.IsNullOrEmpty(result))
                {
                    return "";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether certificate authentication is enabled.
        /// </summary>
        public static bool CertificateAuthenticationRequired => bool.Parse(ApiConstants.CertificateAuthenticationRequiredEnv);

        /// <summary>
        /// Gets a value indicating whether self signed certificates are allowed as the root/intermediate certificate.
        /// </summary>
        public static bool CertificateAuthenticationSelfSigned => bool.Parse(ApiConstants.CertificateAuthenticationSelfSignedEnv);

        /// <summary>
        /// Gets Subject name of the root/intermediate certificate.
        /// </summary>
        public static string CertificateAuthenticationSubjectName => ApiConstants.CertificateAuthenticationSubjectNameEnv;
    }
}
