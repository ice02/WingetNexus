using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingetNexus.Shared.Models.Dtos;

namespace WingetNexus.Data.DataStores
{
    public class ContentFilesDataStore : IContentFilesDataStore
    {
        private readonly WingetNexusContext _context;
        private readonly ILogger<WingetAppDatastore> _logger;
        private readonly IMapper _mapper;

        public ContentFilesDataStore(WingetNexusContext context, ILogger<WingetAppDatastore> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ContentFileDto> GetContentFileByIdAsync(int id)
        {
            var contentFile = await _context.ContentFiles.FindAsync(id);
            if (contentFile == null)
            {
                return null;
            }
            return _mapper.Map<ContentFileDto>(contentFile);
        }
    }
}
