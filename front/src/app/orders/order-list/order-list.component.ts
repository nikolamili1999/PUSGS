import { Component, Input, OnInit } from '@angular/core';
import { EventService } from 'src/app/Shared/event.service';
import { Order } from '../shared/order.model';
import { OrdersService } from '../shared/orders.service';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss']
})
export class OrderListComponent implements OnInit {

  @Input('filter') set setFilter(filter: string) {
    this.filter = filter;
    console.log(this.filter);
    this.loadData();
  }
  /**
   * Filter na osnovu kog cemo da filtriramo porudzbine
   * Moze biti 'active' i 'history'
   */
  filter: string;

  /**
   * Ucivata sve porudzbine i nakon uspesnog dobaljanja filtira ih
   */
  loadData() {
    this.orderService.getOrders().subscribe(
      data => {
        console.log(data);
        if (this.filter == 'history') {
          this.orders = data.filter(i => i.utcTimeDeliveryExpected != 0 && i.utcTimeDeliveryExpected <= new Date().getTime());
        }
        if (this.filter == 'active') {
          this.orders = data.filter(i => i.utcTimeDeliveryExpected != 0 && i.utcTimeDeliveryExpected > new Date().getTime());
        }
      }
    )
  }
  /**
   * Lista svih porudzbina koje se prikazuju korisniku
   */
  orders: Order[] = [];
  constructor(private orderService: OrdersService, private eventService: EventService) {
  }

  ngOnInit(): void {
    this.eventService.eventObservable.subscribe(
      data => {
        this.loadData();
      }
    )
  }

}
