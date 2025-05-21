using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FinancePlanning.Application.Managers
{
    public class XmlExportManager: IXmlExportManager
    {
        public byte[] GenerateXml(InvestmentExportDto dto)
        {
            var serializer = new XmlSerializer(typeof(InvestmentExportDto));
            using var ms = new MemoryStream();
            serializer.Serialize(ms, dto);
            return ms.ToArray();
        }
    }
}
