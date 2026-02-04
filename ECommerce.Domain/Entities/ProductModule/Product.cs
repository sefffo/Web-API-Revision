namespace ECommerce.Domain.Entities.ProductModule
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } =null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }


        //nav props 

        #region Relationships

        public virtual ProductBrand ProductBrand { get; set; } = null!;
        public int BrandId { get; set; }



        public virtual ProductType ProductType { get; set; }
        public int TypeId { get; set; } 

        #endregion
         

    }
}
