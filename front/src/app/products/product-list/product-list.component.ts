import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/Shared/services/auth.service';
import { Product } from '../shared/product.model';
import { ProductService } from '../shared/product.service';
import { UpdateCreateProductDto } from '../shared/create-product.model';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
  providers: [MessageService]
})
export class ProductListComponent implements OnInit {

  products: Product[] = [];

  role = this.authService.roleStateObservable.value;

  /**
   * Flag koji korisnimo oznacava da li je prikazan dijalog za dodavanje/izmenu proizvoda
   */
  displayModal: boolean;
  /**
   * Ovde skladistimo trenutno selektovan fajl slike
   */
  productFile: any;
  /**
   * Forma koja cuva polja neophodna za dodavnje/izmenu proizvoda
   */
  addForm = new FormGroup({
    // id je neophodan samo za izmenu proizvoda(prilikom dodavanja nije poznat)
    id: new FormControl(0),
    name: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(50)]),
    description: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(500)]),
    price: new FormControl(0, [Validators.required, Validators.min(1)]),
    quantity: new FormControl(0, [Validators.required, Validators.min(0)]),
  });
  isAdding = false;
  /**
   * Flag koji oznacava da li izmenjujemo proizvod ili ga dodajemo
   */
  isEditing = false;
  constructor(private productService: ProductService, private messageService: MessageService, private authService: AuthService) {
    this.loadProducts();
  }

  ngOnInit(): void {
    this.authService.roleStateObservable.subscribe(
      data => {
        this.role = data;
      }
    )
  }

  /**
   * Ucitava sve dostupne proizvode
   */
  loadProducts() {
    this.productService.getProducts().subscribe(
      data => {
        this.products = data;
        console.log(this.products);
      },
      error => {

      }
    )
  }

  /**
   * Prikazuje dialog namenjen za dodavanje novom proizvoda
   */
  showAddDialog() {
    this.addForm.setValue(
      {
        'id': null,
        'description': "",
        'name': "",
        'price': 1,
        'quantity': 1,
      }
    );
    this.isEditing = false;
    this.displayModal = true;
  }
  /**
   * Handler koji prihvata izabranu sliku
   * @param event nova fotografija
   */
  onSelectImage(event) {
    this.productFile = event.currentFiles[0];
  }

  /**
   * Dodavanje novog proizvoda
   */
  addProduct() {
    if (this.addForm.valid) {
      this.isAdding = true;
      let createProductDto = this.addForm.value as UpdateCreateProductDto
      let formData = new FormData();
      formData.append("Name", createProductDto.name);
      formData.append("Description", createProductDto.description);
      formData.append("Price", createProductDto.price.toString());
      formData.append("Quantity", createProductDto.quantity.toString());
      console.log(this.productFile);
      if (this.productFile === undefined) {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: "Please set image" });
        this.isAdding = false;
        return;
      }
      formData.append('ImageFile', this.productFile, this.productFile?.name);
      this.productService.addProduct(formData).subscribe(
        data => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Product added successfully' });
          this.displayModal = false;
          this.loadProducts();
          this.isAdding = false;
          this.productFile = null;
        },
        error => {
          this.isAdding = false;
          this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error.message });
        }
      )
    }
  }

  /**
   * Izmenjuje proizvod sa zadatim vrednostima u formi
   */
  updateProduct() {
    if (this.addForm.valid) {
      this.isAdding = true;
      let createProductDto = this.addForm.value as UpdateCreateProductDto
      let formData = new FormData();
      let id = this.addForm.value['id'];
      if (id == null) return;
      formData.append("Name", createProductDto.name);
      formData.append("Description", createProductDto.description);
      formData.append("Price", createProductDto.price.toString());
      formData.append("Quantity", createProductDto.quantity.toString());
      console.log(this.productFile);
      if (this.productFile !== undefined) {
        formData.append('ImageFile', this.productFile!, this.productFile?.name);
      }
      this.productService.updateProduct(id, formData).subscribe(
        data => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Product updated successfully' });
          this.displayModal = false;
          this.loadProducts();
          this.isAdding = false;
          this.productFile = null;
        },
        error => {
          this.isAdding = false;
          this.messageService.add({ severity: 'error', summary: 'Error', detail: "Unable to save changes" });
        }
      )
    }
  }

  /**
   * Handler za slucaj kada se proizvod izbrise
   * @param productId id proizvoda
   */
  onProductDeleted(productId: number) {
    this.deleteProductFromList(productId);
  }

  /**
   * Proizvod sa zadatim id-em brisemo sa liste proizvoda
   * @param productId id proizvoda koji je obrisan
   */
  deleteProductFromList(productId: number) {
    const index: number = this.products.findIndex(i => i.id == productId);
    if (index !== -1) {
      this.products.splice(index, 1);
    }
  }

  /**
   * Handler koji obradjuje zahtev za izmenu proizvoda tj. otvara dijalog za izmenu i popunjava formu sa podacima
   * vezanim za proizvod
   * @param product proizvod koji treba izmeniti
   */
  onProductEdit(product: Product) {
    console.log(product);
    this.isEditing = true;
    this.addForm.setValue(
      {
        'id': product.id,
        'description': product.description,
        'name': product.name,
        'price': product.price,
        'quantity': product.quantity,
      }
    )
    this.isAdding = false;
    this.displayModal = true;
  }

}
