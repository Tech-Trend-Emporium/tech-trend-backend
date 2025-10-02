using Application.Abstraction;
using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
