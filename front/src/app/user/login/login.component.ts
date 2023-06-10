import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { GoogleLoginProvider, SocialAuthService, SocialUser } from 'angularx-social-login';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/Shared/services/auth.service';
import { UserService } from '../shared/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [MessageService, SocialAuthService]
})
export class LoginComponent implements OnInit {

  isProccessing = false;

  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(50)]),
    password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(30)])
  });

  isLoading = false;

  constructor(private userService: UserService, 
    private messageService: MessageService, 
    private router: Router, 
    private authService: AuthService,
    private socialAuthService:SocialAuthService
    ) { }

  ngOnInit(): void {
  }

  login(){
    this.isLoading = true;
    this.userService.login(this.loginForm.value).subscribe(
      data => {
        this.isLoading = false;
        console.log(data);
        this.authService.loginUser(data);
        this.messageService.add({severity:'success', summary: 'Success', detail: 'Login successfull'});
        this.router.navigateByUrl('/home');
      },
      (error) => {
        this.isLoading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: error.error.message});
        console.log(error);
      }
    )
  }

  loginWithGoogle():void {
    this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID)
      .then(() => {
        let sub = this.socialAuthService.authState.subscribe((user:SocialUser) => {
          if(!this.isProccessing){
            this.isProccessing = true;
            this.userService.googleLogin(user).subscribe(
              data => {
                this.isLoading = false;
                console.log(data);
                this.authService.loginUser(data);
                this.messageService.add({severity:'success', summary: 'Success', detail: 'Login successfull'});
                this.router.navigateByUrl('/home');
                this.isProccessing = false;
              },
              (error) => {
                this.isLoading = false;
                this.messageService.add({severity:'error', summary: 'Error', detail: error.error.message});
                console.log(error);
                this.isProccessing = false;
              }
            );
          }
          
      });
    });
  }
}
