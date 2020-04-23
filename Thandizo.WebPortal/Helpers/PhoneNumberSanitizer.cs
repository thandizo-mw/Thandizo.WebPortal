using System;

namespace Thandizo.WebPortal.Helpers
{
    public static class PhoneNumberSanitizer
    {
        public static string Sanitize(string phoneNumber, string preceeder)
        {
            var result = "";
            try
            {
                var firstChar = phoneNumber.Substring(0, 1);

                //Check if its zero that means the user entered, we replace first number only
                if (firstChar.Equals("0"))
                {
                    result = preceeder + phoneNumber.Substring(1);
                }
                //Check if its a two that means the user entered with a 265, we replace first three numbers only
                else if (firstChar.Equals("2"))
                {
                    result = preceeder + phoneNumber.Substring(3);
                }
                //Check if its a plus that means the user entered with a 265, we replace first three numbers only
                else if (firstChar.Equals("+"))
                {
                    result = preceeder + phoneNumber.Substring(4);
                }
                //Otherwise just append the +265 to the whole string
                else
                {
                    result = preceeder + phoneNumber;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                //Do nothing, the result will return the empty string
            }

            return result;
        }
    }
}
