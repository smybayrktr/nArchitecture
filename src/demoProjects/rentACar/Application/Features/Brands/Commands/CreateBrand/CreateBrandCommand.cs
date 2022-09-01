using Application.Features.Brands.Dtos;
using Application.Features.Brands.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommand: IRequest<CreatedBrandDto>
    {
        public string Name { get; set; }

        //İlk handle edilecek ikincisi de dönecek yanıt olarak yazılır
        public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, CreatedBrandDto>
        {
            private readonly IBrandRepository _brandRepository;
            private readonly IMapper _mapper;
            private readonly BrandBusinessRules _brandBusinessRules;

            public CreateBrandCommandHandler(IBrandRepository brandRepository,
                BrandBusinessRules brandBusinessRules,
                IMapper mapper)
            {
                _brandRepository = brandRepository;
                _mapper = mapper;
                _brandBusinessRules = brandBusinessRules;
            }


            //IRequestHandler ın içinde Handle metotu var onu implement ediyoruz. 
            public async Task<CreatedBrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
            {
                await _brandBusinessRules.BrandNameCanNotBeDuplicatedWhenInserted(request.Name);
                //Gelen requesti Brand nesnesine çevircez. Veritabanına göndereceğimiz şey Brand olmalı
                Brand mappedBrand = _mapper.Map<Brand>(request);
                //Veritabanından dönen Brand
                Brand createdBrand = await _brandRepository.AddAsync(mappedBrand);
                //Veritabanından geleni olduğu gibi döndürmiycez göstermek istediğimiz kadarını dto ile döndürcez
                CreatedBrandDto createdBrandDto = _mapper.Map<CreatedBrandDto>(createdBrand);
                //Mapper Dto daki field larla Branddaki fieldları eşlememize yarar.
                return createdBrandDto;
            }
        }
    }
}
