import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, CreateInvoiceRequest } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private apiUrl = 'http://localhost:5000/api/invoices';
  private defaultUserId = '00000000-0000-0000-0000-000000000001';

  constructor(private http: HttpClient) { }

  getInvoices(userId: string = this.defaultUserId): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(`${this.apiUrl}/user/${userId}`);
  }

  getInvoiceById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  createInvoice(request: CreateInvoiceRequest, userId: string = this.defaultUserId): Observable<Invoice> {
    const headers = new HttpHeaders({
      'X-User-Id': userId
    });
    return this.http.post<Invoice>(this.apiUrl, request, { headers });
  }
}
