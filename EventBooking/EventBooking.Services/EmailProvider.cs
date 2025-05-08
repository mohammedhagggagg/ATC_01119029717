using System.Net;
using System.Net.Mail;
using EventBooking.DAL.Data;
using EventBooking.DAL.Services.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventBooking.Services
{
    public class EmailProvider : IEmailProvider
    {
        private readonly IConfiguration _config;
        private readonly EventBookingDbContext _context;

        public EmailProvider(IConfiguration config, EventBookingDbContext context)
        {
            _context = context;
            _config = config;
        }



        public async Task<int> SendResetCode(string Email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null) return 0;

            string subject;
            string templatePath;

            Random rnd = new Random();
            int pin = rnd.Next(1, 999999);
            string pinStr = pin.ToString().PadLeft(6,'0');
            //while (pinStr.Length < 6)
            //{
            //    pinStr = "0" + pinStr;
            //}

            subject = "Vervify email";
            templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email.html";

            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            htmlTemplate = htmlTemplate.Replace("{UserName}", user.UserName);
            htmlTemplate = htmlTemplate.Replace("{VerificationCode}", pinStr);

            var message = new MailMessage
            {
                From = new MailAddress("mohammedhaggagg@gmail.com"),
                Subject = subject,
                Body = htmlTemplate,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.Email));    

            using (var smtp = new SmtpClient(_config["smtp:Host"], int.Parse(_config["Smtp:Port"])))
            {
                smtp.Credentials = new NetworkCredential("mohammedhaggagg@gmail.com", "udpm iayq ymtf adov");
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    return -1;
                }
            }

            return pin;
        }

        public async Task<string> SendConfirmAccount(string Email, string UrlConfirmation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null) return "User Not Found";

            string subject = "Confirm Your Email Address";
            string templatePath = Directory.GetCurrentDirectory() + "/wwwroot/EmailConfirm.html";

            string htmlTemplate = File.ReadAllText(templatePath);
            htmlTemplate = htmlTemplate.Replace("{UserName}", user.DisplayName ?? user.UserName ?? "User"); 
            htmlTemplate = htmlTemplate.Replace("{UrlConfirmation}", UrlConfirmation);

            var message = new MailMessage
            {
                From = new MailAddress("mohammedhaggagg@gmail.com"),
                Subject = subject,
                Body = htmlTemplate,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.Email));

            using (var smtp = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]))) 
            {
                smtp.Credentials = new NetworkCredential("mohammedhaggagg@gmail.com", "udpm iayq ymtf adov");
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                    return "Done";
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    return "Error"; 
                }
            }
        }



        public async Task<string> SendConfirmAccount00(string Email, string UrlConfirmation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null) return "User Not Found";

            string subject = "Confirm Your Email Address";
            string templatePath;
            string body;

            subject = "Confirm Your Email Address";
            //body = $"Hi {user.DisplayName},\r\n\r\nThank you for registering with Handmade E-Commerce!\r\nTo complete your registration and verify your email address, please click the link below:" +
            //    $"{UrlConfirmation}";
            templatePath = Directory.GetCurrentDirectory() + "/wwwroot/EmailConfirm.html";
            //D:\Final\ECommerce\ECommerce.DashBoard\wwwroot\EmailConfirm.html
            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            htmlTemplate = htmlTemplate.Replace("{user.DisplayName}", user.DisplayName);
            htmlTemplate = htmlTemplate.Replace("{UrlConfirmation}", UrlConfirmation);

            var message = new MailMessage
            {
                From = new MailAddress("mohammedhaggagg@gmail.com"),
                Subject = subject,
                Body = htmlTemplate,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(user.Email));

            using (var smtp = new SmtpClient(_config["stmp:Host"], int.Parse(_config["Smtp:Port"])))
            {
                smtp.Credentials = new NetworkCredential("mohammedhaggagg@gmail.com", "udpm iayq ymtf adov");
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    return "Error sending email";
                }
            }

            return "Done";
        }
    }
}
