import { HttpEventType } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { MessageService } from 'primeng/api';
import { User } from '../../shared/user.model';
import { UserService } from '../../shared/user.service';

@Component({
  selector: 'app-seller',
  templateUrl: './seller.component.html',
  styleUrls: ['./seller.component.scss'],
  providers: [MessageService]
})
export class SellerComponent implements OnInit {

  @Input() set userSeller(user: User) {
    this.seller = user;
    this.userService.downloadImage(user.email).subscribe(
      data => {
        if (data.type == HttpEventType.Response) {
          const downloadedFile = new Blob([data!.body!], { type: data!.body!.type });
          let objectURL = URL.createObjectURL(downloadedFile);
          this.profileImageSrc = this.sanitizer.bypassSecurityTrustUrl(objectURL);
        }
      }
    )
  }
  seller: User;
  isLoadingApprove = false;
  isLoadingDeny = false;
  profileImageSrc: any = "https://www.business2community.com/wp-content/uploads/2017/08/blank-profile-picture-973460_640.png";

  constructor(private userService: UserService, private messageService: MessageService, private sanitizer: DomSanitizer) { }

  ngOnInit(): void {
  }

  approve() {
    this.isLoadingApprove = true;
    this.userService.approveSeller(this.seller.email).subscribe(
      data => {
        this.seller.sellerStatus = 2;
        this.isLoadingApprove = false;
      },
      error => {
        this.isLoadingApprove = false;
      }
    )
  }

  deny() {
    this.isLoadingDeny = true;
    this.userService.denySeller(this.seller.email).subscribe(
      data => {
        this.seller.sellerStatus = 3;
        this.isLoadingDeny = false;
      },
      error => {

        this.isLoadingDeny = false;
      }
    )
  }

}
