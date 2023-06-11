import { Product } from "src/app/products/shared/product.model";
/**
 * Cuva informacije o artiklu koji se nalazi u korpi
 */
export class BasketItem{
    productId: number = 0;
    quantity: number = 0;
    product?: Product = null;
}