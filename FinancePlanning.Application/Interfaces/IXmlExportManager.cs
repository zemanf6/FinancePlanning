﻿using FinancePlanning.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Interfaces
{
    public interface IXmlExportManager
    {
        byte[] GenerateXml(InvestmentExportDto dto);
    }
}
