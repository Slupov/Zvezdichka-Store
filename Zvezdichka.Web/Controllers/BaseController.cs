﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Zvezdichka.Web.Infrastructure.Extensions.Helpers;
using Zvezdichka.Web.Infrastructure.Extensions.Helpers.Html;

namespace Zvezdichka.Web.Controllers
{
    public class BaseController : Controller
    {
        public void Success(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
        }

        public void Information(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        public void Warning(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        public void Danger(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = this.TempData.ContainsKey(Alert.TempDataKey)
                ? this.TempData.Get<List<Alert>>(Alert.TempDataKey)
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            this.TempData.Put(Alert.TempDataKey, alerts);
        }
    }
}