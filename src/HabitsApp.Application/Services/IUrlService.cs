using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Application.Services;
public interface IUrlService
{
    string GetAbsoluteUrl(string relativePath);
}
