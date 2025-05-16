import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

 constructor(
    private _HttpClient: HttpClient,
    private _CookieService: CookieService
  ) {}

  private getAuthHeaders(): HttpHeaders {
    const storedData = this._CookieService.get('userData');
    if (!storedData) {
      return new HttpHeaders();
    }
    const parsedData = JSON.parse(storedData);
    if (parsedData && parsedData.token) {
      return new HttpHeaders({
        Authorization: `Bearer ${parsedData.token}`,
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders();
  }
  getAllCategories(): Observable<any> {
    return this._HttpClient.get(`${environment.baseURL}/Category/GetAllCategories`, {
      headers: this.getAuthHeaders()
    });
  }
}
