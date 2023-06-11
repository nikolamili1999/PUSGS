import { Component, Input, EventEmitter, OnInit, Output } from '@angular/core';
import { MessageService } from 'primeng/api';
import { roleGetter } from 'src/app/app.module';
import { BasketItem } from 'src/app/basket/shared/basket-item.model';
import { BasketService } from 'src/app/basket/shared/basket.service';
import { Product } from '../shared/product.model';
import { environment } from 'src/environments/environment';
import { ProductService } from '../shared/product.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
  providers: [MessageService]
})
export class ProductComponent implements OnInit {


  @Input() set prod(product: Product) {
    this.product = product;
    this.imageUrl = environment.serverUrl + '/product/' + product.id + "/image";
  }

  /**
   * Izlazni dogadjaj koji obavestava roditeljsku komponentu da zelimo da izbrisemo proizvod
   */
  @Output() onProductDeleted: EventEmitter<number> = new EventEmitter();
  /**
   * Izlazni dogadjaj koji obavestava roditeljsku komponentu da zelimo da izmenimo proizvod
   */
  @Output() onProductUpdate: EventEmitter<Product> = new EventEmitter();

  product: Product;
  imageUrl: String;

  role = roleGetter();

  constructor(private basketService: BasketService, private messageService: MessageService, private productService: ProductService) {

  }

  ngOnInit(): void {
  }

  /**
   * Dodaje proizvod u korpu
   */
  addToBasket() {
    let item = new BasketItem();
    item.productId = this.product.id;
    item.quantity = 1;
    this.basketService.addToBasket(item).subscribe(
      data => {
        // prikaz toast-a u gornjem desnom uglu
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Product added to basket:)' });
      },
      error => {
        // prikaz toast-a u gornjem desnom uglu
        this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error.message });
      }
    )
  }

  /**
   * Salje dogadjaj koji zahteva brisanje proizvoda
   */
  deleteProduct() {
    this.productService.deleteProduct(this.product.id).subscribe(
      data => {
        this.onProductDeleted.emit(this.product.id);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Product deleted' });
      },
      error => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error.message });
      }
    )
  }

  /**
   * Salje dogadjaj koji zahteva izmenu proizvoda
   */
  editProduct() {
    this.onProductUpdate.emit(this.product);
  }

}
