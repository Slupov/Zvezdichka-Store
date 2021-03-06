﻿using System.Threading.Tasks;

namespace Zvezdichka.Services.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
