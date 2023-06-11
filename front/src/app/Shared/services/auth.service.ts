import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Token } from 'src/app/user/shared/token.model';

@Injectable({
  providedIn: 'root'
})
// Servis za dobavljanje stanja autentikacije tj. donosi nam informacije da li je korisnik ulogovan
// i ako jeste koja je uloga korisnika
export class AuthService {

  public authStateObservable = new BehaviorSubject<boolean>(localStorage.getItem('token') != null);
  public roleStateObservable = new BehaviorSubject<string | null>(localStorage.getItem('role'));

  constructor() { }

  loginUser(token: Token) {
    localStorage.setItem('role', token.role);
    localStorage.setItem('token', token.token);
    this.roleStateObservable.next(token.role);
    this.authStateObservable.next(true);
  }

  logoutUser() {
    localStorage.removeItem('role');
    localStorage.removeItem('token');
    this.roleStateObservable.next(null);
    this.authStateObservable.next(false);
  }

}
