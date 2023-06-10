import { Component, OnInit } from '@angular/core';
import { User } from '../../shared/user.model';
import { UserService } from '../../shared/user.service';

@Component({
  selector: 'app-sellers-list',
  templateUrl: './sellers-list.component.html',
  styleUrls: ['./sellers-list.component.scss']
})
export class SellersListComponent implements OnInit {

  isLoading = true;
  sellers: User[]

  constructor(private userService: UserService) {
    this.userService.getSellers().subscribe(
      data => {
        this.sellers = data;
        this.isLoading = false;
      }
    )
  }

  ngOnInit(): void {
  }

}
