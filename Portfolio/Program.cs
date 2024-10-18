using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Portfolio.Application;
using Portfolio.Model;
using Portfolio.Service.Live;

MainApplication mainApplication = new MainApplication(@"Raw\appSettings.json");
mainApplication.DisplayUserInterface();

