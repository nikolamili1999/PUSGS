import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BasketItem } from './basket-item.model';
import { Basket } from './basket.model';
import { Checkout } from './checkout.model';

@Injectable({
  providedIn: 'root'
})
export class BasketService {

  constructor( private http: HttpClient) { }

  /** Ovde cuvamo broj o ukupnoj vrednosti korpe
  * cuvamo ga kao observabilnu promenjivu kako bi mogli iz razmicitih komponenti da osvezavamo ukupnu sumu
  * svih proizvoda u korpi*/
  public totalObservable = new BehaviorSubject<number>(0);

  /**
   * Postavlja novu vrednost korpe
   * @param basket nova vrednost korpe
   * @returns 
   */
  setBasket(basket: Basket) :Observable<Object> {
    return this.http.post<Object>(environment.serverUrl + '/basket', basket);
  }

  /**
   * Vraca trenutnu korpu korisnika
   * @returns trenutnu korpu korisnika
   */
  getBasket(): Observable<Basket>{
    return this.http.get<Basket>(environment.serverUrl+ '/basket');
  }

  /**
   * Checkout-jemo korpu odnosno kreiramo porudzbinu na osnovu korpe
   * @param checkout - podaci o adresi i komentar porudzbine
   * @returns novu porudzbinu
   */
  checkout(checkout: Object): Observable<Object>{
    return this.http.post<Object>(environment.serverUrl + '/basket/checkout', checkout);
  }

  /**
   * Dodavanje artikla u korpu
   * @param item - item koji dodajemo u korpu
   * @returns 
   */
  addToBasket(item: BasketItem) :Observable<Object> {
    return this.http.put<Object>(environment.serverUrl + '/basket', item);
  }
}
