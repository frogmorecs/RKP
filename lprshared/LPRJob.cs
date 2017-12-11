using System.IO;

namespace lprshared
{
    public class LPRJob
    {
        [Parameter("-S", Required = true)]
        public string Server { get; set; }

        [Parameter("-P", Required = true, AllowSpaces = false)]
        public string Printer { get; set; }

        [Parameter(Default = true)]
        public string Path { get; set; }

        [Parameter("-o")]
        public string FileType { get; set; } = "f";

        [Parameter("-C")]
        public string Class { get; set; }

        [Parameter("-J")]
        public string JobName{ get; set; }

        [Parameter("-d")]
        public bool SendDataFileFirst { get; set; }

        [Parameter("-x")]
        public bool SunOsCompatibility { get; set; } // Ignored

        public Stream InputFile { get; set; }

        protected bool Equals(LPRJob other)
        {
            return string.Equals(Server, other.Server) 
                    && string.Equals(Printer, other.Printer) 
                    && string.Equals(Path, other.Path)
                    && string.Equals(FileType, other.FileType)
                    && string.Equals(Class, other.Class)
                    && string.Equals(JobName, other.JobName)
                    && SendDataFileFirst == other.SendDataFileFirst
                    && SunOsCompatibility == other.SunOsCompatibility;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((LPRJob) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = Server?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Printer?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Path?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (FileType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}