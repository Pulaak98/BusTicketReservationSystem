import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-seat-plan',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './seat-plan.component.html',
  styleUrls: ['./seat-plan.component.css'],
})
export class SeatPlanComponent implements OnInit {
  scheduleId!: string;
  seats: any[] = [];
  selectedSeats: { id: string; number: string }[] = [];
  busName = '';
  companyName = '';
  passengerName = '';
  mobileNumber = '';
  boardingPoints: string[] = [];
  droppingPoints: string[] = [];
  boardingPoint = '';
  droppingPoint = '';
  ticketId: string = '';
  showCancelForm: boolean = false;

  constructor(private route: ActivatedRoute, private api: ApiService) {}

  ngOnInit(): void {
    this.scheduleId = this.route.snapshot.paramMap.get('scheduleId')!;
    this.api.getSeatPlan(this.scheduleId).subscribe((res) => {
      this.busName = res.busName;
      this.companyName = res.companyName;
      this.seats = res.seats.sort((a: any, b: any) => {
  const numA = parseInt(a.seatNumber.replace(/\D/g, ''));
  const numB = parseInt(b.seatNumber.replace(/\D/g, ''));
  return numA - numB;
});

      if (res.boardingPoint) {
        this.boardingPoints = [res.boardingPoint];
        this.boardingPoint = res.boardingPoint;
      }
      if (res.droppingPoint) {
        this.droppingPoints = [res.droppingPoint];
        this.droppingPoint = res.droppingPoint;
      }
    });
  }

  toggleSeat(seat: any) {
    if (seat.status !== 'Available') return;

    const index = this.selectedSeats.findIndex((s) => s.id === seat.seatId);
    if (index > -1) {
      this.selectedSeats.splice(index, 1);
    } else {
      this.selectedSeats.push({ id: seat.seatId, number: seat.seatNumber });
    }
  }


  isSelected(seat: any): boolean {
    return this.selectedSeats.some((s) => s.id === seat.seatId);
  }

  getSeatClass(seat: any): string {
    if (seat.status === 'Available') {
      return this.isSelected(seat)
        ? 'btn btn-success' // selected
        : 'btn btn-outline-success'; // available
    }
    if (seat.status === 'Booked') return 'btn btn-warning';
    if (seat.status === 'Sold') return 'btn btn-danger';
    return 'btn btn-secondary';
  }
  getSeatLayout(): any[][] {
    const layout: any[][] = [];
    for (let i = 0; i < this.seats.length; i += 4) {
      layout.push(this.seats.slice(i, i + 4));
    }
    return layout;
  }

  bookNow() {
    if (this.selectedSeats.length === 0) return;
    console.log(this.selectedSeats);
    const bookingRequest = {
      busScheduleId: this.scheduleId,
      seatId: this.selectedSeats.map((s) => s.id),
      passengerName: this.passengerName,
      passengerMobile: this.mobileNumber, 
      boardingPoint: this.boardingPoint,
      droppingPoint: this.droppingPoint,
      action: 'Book',
    };

    this.api.bookSeats(bookingRequest).subscribe({
      next: (res) => {
        alert('Booking confirmed!');
        // reset form
        this.selectedSeats = [];
        this.passengerName = '';
        this.mobileNumber = '';
        this.boardingPoint = '';
        this.droppingPoint = '';

        // refresh seat plan so UI updates
        this.api.getSeatPlan(this.scheduleId).subscribe((updated) => {
          this.seats = updated.seats.sort((a: any, b: any) => {
  const numA = parseInt(a.seatNumber.replace(/\D/g, ''));
  const numB = parseInt(b.seatNumber.replace(/\D/g, ''));
  return numA - numB;
});

        });
      },
      error: (err) => {
        console.error('Booking failed:', err);
        alert('Booking failed. Please try again.');
      },
    });
  }
  buyNow() {
    if (this.selectedSeats.length === 0) return;
    console.log(this.selectedSeats);
    const bookingRequest = {
      busScheduleId: this.scheduleId, 
      seatId: this.selectedSeats.map((s) => s.id),
      passengerName: this.passengerName,
      passengerMobile: this.mobileNumber, 
      boardingPoint: this.boardingPoint,
      droppingPoint: this.droppingPoint,
      action: 'Buy',
    };

    this.api.buySeats(bookingRequest).subscribe({
      next: (res) => {
        alert('Ticket confirmed!');
        // reset form
        this.selectedSeats = [];
        this.passengerName = '';
        this.mobileNumber = '';
        this.boardingPoint = '';
        this.droppingPoint = '';

        // refresh seat plan so UI updates
        this.api.getSeatPlan(this.scheduleId).subscribe((updated) => {
          this.seats = updated.seats.sort((a: any, b: any) => {
  const numA = parseInt(a.seatNumber.replace(/\D/g, ''));
  const numB = parseInt(b.seatNumber.replace(/\D/g, ''));
  return numA - numB;
});

        });
      },
      error: (err) => {
        console.error('Booking failed:', err);
        alert('Booking failed. Please try again.');
      },
    });
  }
  cancelTicket() {
  this.api.cancelTicket({ ticketId: this.ticketId }).subscribe({
    next: () => {
      alert('Ticket cancelled.');
      this.ticketId = '';
      this.api.getSeatPlan(this.scheduleId).subscribe(updated => {
        this.seats = updated.seats.sort((a: any, b: any) => {
          const numA = parseInt(a.seatNumber.replace(/\D/g, ''));
          const numB = parseInt(b.seatNumber.replace(/\D/g, ''));
          return numA - numB;
        });
      });
    },
    error: () => alert('Cancellation failed.')
  });
}
isValidMobile(mobile: string): boolean {
  const pattern = /^(017|018|016|019|015|013)\d{8}$/;
  return pattern.test(mobile);
}

}
