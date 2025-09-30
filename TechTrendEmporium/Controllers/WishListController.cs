using Application.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishListController
    {
        private readonly IWishListRepository _wishListRepository;

        public WishListController(IWishListRepository wishListRepository)
        {
            _wishListRepository = wishListRepository;
        }
    }
}
