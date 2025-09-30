using Application.Abstraction;
using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WishListService(IWishListRepository wishListRepository, IUnitOfWork unitOfWork)
        {
            _wishListRepository = wishListRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
