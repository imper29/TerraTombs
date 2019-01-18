namespace ModLoading
{
    [System.Serializable]
    public struct ModInfo
    {
        public string displayName;
        public string id;

        public ModVersion modVersion;
        public ModVersion minTargetVersion;
        public ModVersion maxTargetVersion;

        public ModDependency[] dependencies;


        public ModInfo(string displayName, string id, ModVersion modVersion, ModVersion minTargetVersion, ModVersion maxTargetVersion, params ModDependency[] dependencies)
        {
            this.displayName = displayName;
            this.id = id;
            this.modVersion = modVersion;
            this.minTargetVersion = minTargetVersion;
            this.maxTargetVersion = maxTargetVersion;
            this.dependencies = dependencies;
        }
    }
    [System.Serializable]
    public struct ModVersion
    {
        public int year, month, day;


        public ModVersion(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }


        public override string ToString()
        {
            return "v" + year.ToString() + "." + month.ToString() + "." + day.ToString();
        }

        public static bool operator <(ModVersion a, ModVersion b)
        {
            if (a.year > b.year)
                return false;
            if (a.month > b.month)
                return false;

            return !(a.day >= b.day);
        }
        public static bool operator >(ModVersion a, ModVersion b)
        {
            if (a.year < b.year)
                return false;
            if (a.month < b.month)
                return false;

            return !(a.day <= b.day);
        }
        public static bool operator >=(ModVersion a, ModVersion b)
        {
            if (a.Equals(b))
                return true;

            return a > b;
        }
        public static bool operator <=(ModVersion a, ModVersion b)
        {
            if (a.Equals(b))
                return true;

            return a < b;
        }
    }
    [System.Serializable]
    public struct ModDependency
    {
        public string dependencyId;
        public ModVersion minDependencyVersion;
        public ModVersion maxDependencyVersion;
        

        public ModDependency(string targetId, ModVersion minDependencyVersion, ModVersion maxDependencyVersion)
        {
            this.dependencyId = targetId;
            this.minDependencyVersion = minDependencyVersion;
            this.maxDependencyVersion = maxDependencyVersion;
        }


        public override string ToString()
        {
            return dependencyId + " (" + minDependencyVersion.ToString() + "-" + maxDependencyVersion.ToString() + ")";
        }
    }
}