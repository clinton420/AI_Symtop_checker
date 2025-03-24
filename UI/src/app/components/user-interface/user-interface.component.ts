import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-interface',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="user-container">
      <header>
        <h1>AI Symptom Checker</h1>
        <nav>
          <button (click)="logout()">Logout</button>
        </nav>
      </header>
      
      <main>
        <div class="symptom-checker">
          <h2>Check Your Symptoms</h2>
          <div class="symptom-form">
            <div class="form-group">
              <label>Describe your symptoms:</label>
              <textarea [(ngModel)]="symptoms" placeholder="Enter your symptoms..."></textarea>
            </div>
            <div class="form-group">
              <label>Age:</label>
              <input type="number" [(ngModel)]="age" placeholder="Enter your age">
            </div>
            <div class="form-group">
              <label>Gender:</label>
              <select [(ngModel)]="gender">
                <option value="">Select gender</option>
                <option value="male">Male</option>
                <option value="female">Female</option>
                <option value="other">Other</option>
              </select>
            </div>
            <button (click)="checkSymptoms()">Analyze Symptoms</button>
          </div>

          <div class="results" *ngIf="showResults">
            <h3>Analysis Results</h3>
            <p>Based on your symptoms, here are possible conditions:</p>
            <ul>
              <li *ngFor="let result of mockResults">{{ result }}</li>
            </ul>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .user-container {
      min-height: 100vh;
      background: #f8f9fa;
    }
    header {
      background: #4a69bd;
      color: white;
      padding: 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    main {
      padding: 2rem;
      max-width: 800px;
      margin: 0 auto;
    }
    .symptom-checker {
      background: white;
      padding: 2rem;
      border-radius: 10px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .symptom-form {
      margin-top: 2rem;
    }
    .form-group {
      margin-bottom: 1rem;
    }
    label {
      display: block;
      margin-bottom: 0.5rem;
      color: #2c3e50;
    }
    textarea, input, select {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      margin-bottom: 1rem;
    }
    textarea {
      height: 100px;
    }
    button {
      padding: 0.75rem 1.5rem;
      background: #4a69bd;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }
    .results {
      margin-top: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 4px;
    }
  `]
})
export class UserInterfaceComponent {
  symptoms: string = '';
  age: number | null = null;
  gender: string = '';
  showResults: boolean = false;
  mockResults: string[] = [
    'Common Cold - 85% confidence',
    'Seasonal Allergies - 65% confidence',
    'Viral Infection - 45% confidence'
  ];

  checkSymptoms() {
    this.showResults = true;
  }

  logout() {
    // Implement logout logic
  }
}