using Application.Dtos.Product;
using Data.Entities;
using General.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace General.Mappers
{
    public static class ProductMapper
    {
        public static Product createToEntity(CreateProductRequest dto, CancellationToken ct)
        {
            var product = new Product
            {
                Title = dto.title,
                Price = dto.price,
                Description = dto.description,
                ImageUrl = dto.image,
                RatingRate = dto.rating.rate,
                Count = (int)dto.rating.count,
                CategoryId = int.Parse(dto.category.Split('/').Last())
            };
            product.Inventory.Total = dto.inventory.total;
            product.Inventory.Available = dto.inventory.available;

            return product;
        }
        public static Product updateToEntity(UpdateProductRequest dto, CancellationToken ct)
        {
            var product = new Product
            {
                Id = dto.Id,
                Title = dto.title,
                Price = dto.price,
                Description = dto.description,
                ImageUrl = dto.image,
                RatingRate = dto.rating.rate,
                Count = (int)dto.rating.count,
                CategoryId = int.Parse(dto.category.Split('/').Last())
            };
            product.Inventory.Total = dto.inventory.total;
            product.Inventory.Available = dto.inventory.available;

            return product;
        }
        public static List<ProductResponse> ToResponseList(IReadOnlyList<Product> products)
        {
            var responseList = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                title = p.Title,
                price = p.Price,
                category = p.Category.Name + "/" + p.CategoryId,
                
            }).ToList();
            return responseList;
        }

        public static ProductResponse ToResponse(Product product)
        {
            var response = new ProductResponse
            {
                Id = product.Id,
                title = product.Title,
                price = product.Price,
                category = product.Category.Name + "/" + product.CategoryId.ToString(),

            };
            return response;
        }

        public static UpdateProductResponse UpdateResponse(bool state)
        {
            if (state) 
            {
                return  new UpdateProductResponse { Message = "Updated Successfully" };
            }
            else
            {
                return new UpdateProductResponse { Message = "Updated failed" };
            }
        }

        public static CreateProductResponse CreateResponse(bool state, int id)
        {
            if (state)
            {
                return new CreateProductResponse { productId = id, Message = "Successful" };
            }
            else
            {
                return new CreateProductResponse { productId = id, Message = "Failure" };
            }
        }

        public static DeleteProductResponse DeleteResponse(bool state)
        {
            if (state)
            {
                return new DeleteProductResponse { Message = "Deleted Successfully" };
            }
            else
            {
                return new DeleteProductResponse { Message = "Delete failed, product not found" };
            }
        }
    }
}
