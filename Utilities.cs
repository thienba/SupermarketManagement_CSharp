using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace SupermarketManagement
{
    class Utilities
    {
        internal static void MessageInfo(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        internal static void MessageWarning(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        internal static DialogResult MessageYesNo(string message, string caption)
        {
            DialogResult dlr = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            return dlr;
        }
        internal static bool IsPhoneNumber(string number)
        {
            bool Result = Regex.Match(number, @"^(03|05|07|08|09|01[2|6|8|9])+([0-9]{8})$").Success;
            if (!Result)
            {
                MessageWarning("Phone No invalidate.");
                return false;
            }
            return true;
        }

        internal static bool IsValidEmail(string eMail)
        {
            try
            {
                MailAddress mail = new MailAddress(eMail);

                return true;
            }
            catch (FormatException)
            {
                MessageWarning("Email invalidate.");
                return false;
            }
        }
        internal static bool IsValidUsername(string Username)
        {
            bool Result = Regex.Match(Username, @"^[A-Za-z0-9_\.]{6,32}$").Success;
            if (!Result)
            {
                MessageWarning("Username invalidate.");
                return false;
            }
            return true;
        }
    }
}
