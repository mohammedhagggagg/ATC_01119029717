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
   private getAuthHeaders(contentType: string = 'application/json'): HttpHeaders {
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
}  
getEventById(id: number): Observable<any> {
  const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  console.log('API URL:', `${environment.baseURL}/Event/GetEventById/${id}`);
  
  return this._HttpClient.get(`${environment.baseURL}/Event/GetEventById/${id}`, { headers: this.getAuthHeaders() });
}

// Create: Create a New Event
  createEvent(eventData: any, photos: File[]): Observable<any> {
    const formData = new FormData();
    formData.append('Title', eventData.title);
    formData.append('Description', eventData.description);
   const eventDate = typeof eventData.date === 'string' ? new Date(eventData.date) : eventData.date;
    formData.append('Date', eventDate.toISOString());
    formData.append('Venue', eventData.venue);
    formData.append('Price', eventData.price.toString());
    formData.append('CategoryId', eventData.categoryId.toString());

    // Append photos to formData
    if (photos && photos.length > 0) {
      photos.forEach((photo, index) => {
        formData.append(`Photos`, photo, photo.name);
      });
    }
console.log('Photos being uploaded:', photos);

    return this._HttpClient.post(`${environment.baseURL}/Event/CreateEvent`, formData, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${JSON.parse(this._CookieService.get('userData')).token}`
      })
    });
  }

  // Update: Update an Existing Event
  updateEvent(id: number, eventData: any, photosToAdd: File[], photoIdsToDelete: number[]): Observable<any> {
    const formData = new FormData();
    formData.append('Title', eventData.title);
    formData.append('Description', eventData.description);
   const eventDate = typeof eventData.date === 'string' ? new Date(eventData.date) : eventData.date;
    formData.append('Date', eventDate.toISOString());
    formData.append('Venue', eventData.venue);
    formData.append('Price', eventData.price.toString());
    formData.append('CategoryId', eventData.categoryId.toString());

  
    if (photosToAdd && photosToAdd.length > 0) {
      photosToAdd.forEach((photo, index) => {
        formData.append(`PhotosToAdd`, photo, photo.name);
      });
    }

   
    // if (photoIdsToDelete && photoIdsToDelete.length > 0) {
    //   photoIdsToDelete.forEach((id, index) => {
    //     formData.append(`PhotoIdsToDelete[${index}]`, id.toString());
    //   });
    // }
   if (photoIdsToDelete && photoIdsToDelete.length > 0) {
  photoIdsToDelete.forEach((id, index) => {
    formData.append(`PhotoIdsToDelete[${index}]`, id.toString());
  });
} 
    
    return this._HttpClient.put(`${environment.baseURL}/Event/UpdateEvent/${id}`, formData, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${JSON.parse(this._CookieService.get('userData')).token}`
      })
    });
  }

  // Delete: Delete an Event
  deleteEvent(id: number): Observable<any> {
    return this._HttpClient.delete(`${environment.baseURL}/Event/DeleteEvent/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  // Restore: Restore a Deleted Event
  // restoreEvent(id: number): Observable<any> {
  //   return this._HttpClient.put(`${environment.baseURL}/Event/RestoreEvent/${id}`, {}, {
  //     headers: this.getAuthHeaders()
  //   });
  // }
}
