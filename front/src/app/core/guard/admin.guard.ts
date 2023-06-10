import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

import {JwtHelperService} from '@auth0/angular-jwt';
@Injectable({
  providedIn: 'root'
})

export class AdminGuard implements CanActivate {
  constructor(private router: Router, private helper:JwtHelperService) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      let token = localStorage.getItem('token');
      if(token != null)
      {
        let role = localStorage.getItem('role'); 

        if(!this.helper.isTokenExpired(token) && role=='ADMIN') { 
          return true;
        }

        this.router.navigateByUrl('/home');
        return false;
      }
      this.router.navigateByUrl('/login');
      return false;
  }
}
