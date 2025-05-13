import { environment } from './../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private _HttpClient:HttpClient,private _CookieService : CookieService, private _auth: AuthService) {

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
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders();
  }

  getAllEvents(
    pageSize: number,
    pageIndex: number,
    categoryId?: number | null,
    maxPrice?: number | null,
    minPrice?: number | null,
    startDate?: string | null,
    endDate?: string | null,
  ): Observable<any> 
{
  const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const params: any = {
      pageSize: pageSize,
      pageIndex: pageIndex,
    };
    if (categoryId) {
      params.categoryId = categoryId;
    }
    if (maxPrice) {
      params.maxPrice = maxPrice;
    }
    if (minPrice) {
      params.minPrice = minPrice;
    }
    if (startDate) {
      params.startDate = startDate;
    }
    if (endDate) {
      params.endDate = endDate;
    }
    console.log('API URL:', `${environment.baseURL}/Event/GetAllWithFilter`);
  console.log('Request Headers:', headers);
  console.log('Request Params:', params);
    return this._HttpClient.get(`${environment.baseURL}/Event/GetAllWithFilter`, { headers: this.getAuthHeaders(), params });
    console.log(`${environment.baseURL}`);
    
}  
getEventById(id: number): Observable<any> {
  const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  console.log('API URL:', `${environment.baseURL}/Event/GetEventById/${id}`);
  
  return this._HttpClient.get(`${environment.baseURL}/Event/GetEventById/${id}`, { headers: this.getAuthHeaders() });
}
}
