import { Routes } from '@angular/router';
import { SearchComponent } from './components/search/search.component';
import { SeatPlanComponent } from './components/seat-plan/seat-plan.component';
// import { BookingComponent } from './components/booking/booking.component';
// import { TicketsComponent } from './components/tickets/tickets.component';

export const routes: Routes = [
  { path: '', redirectTo: 'search', pathMatch: 'full' },
  { path: 'search', component: SearchComponent },
  { path: 'seatplan/:scheduleId', component: SeatPlanComponent },
  // { path: 'booking/:scheduleId', component: BookingComponent },
  // { path: 'tickets/:mobile', component: TicketsComponent }
];
