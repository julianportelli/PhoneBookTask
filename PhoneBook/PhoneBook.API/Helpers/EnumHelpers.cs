namespace PhoneBook.API.Helpers
{
    public static class EnumHelpers
    {
        public static T ParseEnumCustom<T>(string enumString)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), enumString);
            }
            catch (Exception ex)
            {
                string enumValues = string.Join(", ", Enum.GetNames(typeof(T)));

                throw new ArgumentException($"The action \"{enumString}\" is not supported. Only {enumValues} are supported.");
            }
        }
    }
}
