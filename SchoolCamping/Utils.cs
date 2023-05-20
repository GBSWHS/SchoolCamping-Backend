namespace SchoolCamping
{
    public static class Utils
    {
        public static string Mask(this string val)
        {
            string v = "";
            foreach (var value in val.Split(' '))
            {
                v += $"{value[0]}{string.Join("", Enumerable.Repeat("*", value.Length - 1))} ";
            }
            return v.Trim();
        }
    }
}
