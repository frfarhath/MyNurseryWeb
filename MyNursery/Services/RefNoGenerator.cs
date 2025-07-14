using System;
using System.Text.RegularExpressions;

namespace MyNursery.Services
{
    public class RefNoGenerator
    {
        public string Generate(string childFirstName, string childLastName)
        {
            // Sanitize and combine initials
            string initials = $"{GetInitials(childFirstName)}{GetInitials(childLastName)}";

            // Generate timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Combine and return RefNo
            return $"LSN-{initials}-{timestamp}";
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "X";
            name = Regex.Replace(name.ToUpper(), @"[^A-Z]", ""); // Remove non-letters
            return name.Length >= 2 ? name.Substring(0, 2) : name;
        }
    }
}
