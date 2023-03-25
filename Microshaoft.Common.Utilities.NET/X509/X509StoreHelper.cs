namespace Microshaoft
{
    using System.Security.Cryptography.X509Certificates;
    public static class X509StoreHelper
    {
        public static X509Certificate2 GetCertificate(this X509Store @this, string subjectName = null!)
        {
            X509Certificate2 x509Certificate2 = null!;
            if
                (
                    !string.IsNullOrEmpty(subjectName)
                    &&
                    !string.IsNullOrWhiteSpace(subjectName)
                )
            {
                if (subjectName.StartsWith("CN="))
                {
                    subjectName = subjectName["CN=".Length..];
                }
                try
                {
                    if (@this.IsOpen)
                    {
                        @this.Close();
                    }
                    @this
                        .Open(OpenFlags.ReadOnly);
                    var x509FindType = X509FindType
                                                .FindBySubjectName;
                    var x509CertificateCollection = @this
                                                            .Certificates
                                                            .Find
                                                                (
                                                                    X509FindType.FindByTimeValid
                                                                    , DateTime.UtcNow
                                                                    , false
                                                                )
                                                            .Find
                                                                (
                                                                    x509FindType
                                                                    , subjectName
                                                                    , false
                                                                );
                    x509Certificate2 = x509CertificateCollection
                                                            .Cast<X509Certificate2>()
                                                            .OrderByDescending
                                                                    (x => x.NotBefore)
                                                            .FirstOrDefault()!;
                }
                finally
                {
                    if (@this.IsOpen)
                    {
                        @this.Close();
                    }
                }

            }
            return x509Certificate2;
        }
    }
}