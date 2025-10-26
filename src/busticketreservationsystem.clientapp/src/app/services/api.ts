import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = 'https://localhost:7196/api'; // adjust to your backend port

  constructor(private http: HttpClient) {}

  searchBuses(from: string, to: string, date: string) {
    return this.http.get<any[]>(`${this.baseUrl}/Search`, { params: { from, to, date } });
  }

  getSeatPlan(scheduleId: string) {
    return this.http.get<any>(`${this.baseUrl}/Booking/seatplan/${scheduleId}`);
  }

  bookSeats(dto: any) {
    return this.http.post(`${this.baseUrl}/Booking/book`, dto);
  }

  buySeats(dto: any) {
    return this.http.post(`${this.baseUrl}/Booking/buy`, dto);
  }

  cancelTicket(dto: any) {
    return this.http.post(`${this.baseUrl}/Booking/cancel`, dto);
  }
}
