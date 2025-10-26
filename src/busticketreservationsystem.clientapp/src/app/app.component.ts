import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container-fluid">
        <a class="navbar-brand" routerLink="/">BusTicketReservation</a>
        <button
          class="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <button class="btn btn-link nav-link" (click)="goTo('/')">Home</button>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/#">About</a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
    <h1 class="text-center mt-3">Bus Ticket Reservation System</h1>
    <div class="bg-overlay">
      <router-outlet></router-outlet>
    </div>
  `,
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  constructor(private router: Router) {}

  goTo(path: string) {
    this.router.navigate([path]);
  }
}
