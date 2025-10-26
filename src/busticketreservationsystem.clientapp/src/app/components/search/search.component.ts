import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent {
  cities: string[] = [
    'Dhaka',
    'Mymensingh',
    'Chattogram',
    'Sylhet',
    'Rajshahi',
    'Khulna',
    'Barishal',
    'Rangpur',
    'Cox’s Bazar',
    'Comilla',
  ];
  from = '';
  to = '';
  date = '';
  results: any[] = [];
  minDate: string = '';
  searched = false;

  ngOnInit() {
    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    this.minDate = `${yyyy}-${mm}-${dd}`;
  }

  constructor(private api: ApiService) {}

  onSearch() {
    if (this.from === this.to) {
      alert('Origin and destination cannot be the same.');
      return;
    }
    console.log('Search clicked:', this.from, this.to, this.date);
    const selectedDate = new Date(this.date);
    const formattedDate = selectedDate.toISOString().split('T')[0]; // '2025-10-26'
    this.searched = false;
    this.api.searchBuses(this.from, this.to, formattedDate).subscribe({
      next: (res) => {
        console.log('API response:', res);
        this.results = res;
        this.searched = true;
      },
      error: (err) => {
        console.error('API error:', err);
        this.results = [];
        this.searched = true;
      },
    });
  }
}
