import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Order } from './order.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {

  constructor(private http: HttpClient) { }

  /**
   * Vraca sve porudzbine dostupne korisniku na uvid u zavisnosti od uloge
   * @returns porudzbine
   */
  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(environment.serverUrl + "/orders");
  }
}
