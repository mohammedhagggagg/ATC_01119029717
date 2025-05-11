import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  userData = new BehaviorSubject<any>(null);
  // theme : string = 'light';
  themeSubject = new BehaviorSubject<string>('light');
  constructor(private _HttpClient:HttpClient,private Router:Router,private _CookieService :CookieService) 
  {
    this.loadUserData();
    this.checktheme();
   }

  saveUserData(response: any) {
    const userData = {
      token: response.token,
      userId: response.userId,
      displayName: response.displayName,
      image: response.image,
    };

    this._CookieService.set('userData', JSON.stringify(userData), {
      expires: 2,
      path: '/',
    });

    this.userData.next(userData);
    this.checktheme();
  }
  checktheme(): void {
    const theme = this._CookieService.get('theme');
    if (!theme) {
      this._CookieService.set('theme', 'light', {
        expires: 30,
        path: '/',
      });
      this.themeSubject.next('light');
    } else {
      this.themeSubject.next(theme);
    }
  }


  loadUserData() {
    const storedData = this._CookieService.get('userData');
    if (storedData) {
      const parsedData = JSON.parse(storedData);
      if (parsedData.token) {
        this.userData.next(parsedData);
      } else {
        this.signout();
      }
    }
  }
  private getAuthHeaders(): HttpHeaders {
    const storedData = this._CookieService.get('userData');
    if (!storedData) {
      return new HttpHeaders();
    }
    const parsedData = JSON.parse(storedData);
    if (parsedData && parsedData.token) {
      return new HttpHeaders({
        Authorization: `Bearer ${parsedData.token}`,
      });
    }
    return new HttpHeaders();
  }signout() {
    this._CookieService.delete('userData', '/');
    this._CookieService.delete('theme', '/');
    this.userData.next(null);
   this.themeSubject.next('light');
    window.location.href = '/home';
  }signup(
    credentials:{
      displayName: string;
      userName: string;
      phoneNumber: string;
      email: string;
      password: string;
      confirmPassword: string;
    }): Observable<any>
    {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/register`, credentials, { headers });
  }

  login(credentials: { emailOrUserName: string; password: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    console.log('Sending login request to:', `${environment.baseURL}Account/login`, 'with data:', credentials); 
    return this._HttpClient.post(`${environment.baseURL}Account/login`, credentials, { headers });
  }

  SendPinCode(credentials: { email: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/send_reset_code`, credentials, { headers });
  }

  Verify_Pin( email : string, credentials: { pin: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/verify_pin/${email}`, credentials, { headers });
  }

  ForgetPassword( email : string, credentials: { newPassword: string, confirmNewPassword: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/forget_password/${email}`, credentials, { headers });
  }

  ChangePassword( credentials: { oldPassword: string, newPassword: string, confirmPassword: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Account/change_password`, credentials, { headers: this.getAuthHeaders() });
  }
}
