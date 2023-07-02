using AutoMapper;
using GameManagement_.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameManagement_.Api.Controllers
{
    [Route("api/publishers")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public PublishersController(IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.mapper = mapper;
            this.propertyMappingService = propertyMappingService;
            this.propertyCheckerService = propertyCheckerService;
        }






    }
}
