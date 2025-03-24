import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PredictionService } from '../../services/prediction.service';
import { SymptomCheckPrediction } from '../../models/prediction.model';

@Component({
  selector: 'app-symptom-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="symptom-form-container">
      <form [formGroup]="checkForm" (ngSubmit)="onSubmit()" class="needs-validation">
        <div class="mb-4">
          <label class="form-label">Symptoms</label>
          <div class="input-group">
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="currentSymptom"
              [ngModelOptions]="{standalone: true}"
              placeholder="Enter a symptom"
            >
            <button type="button" class="btn btn-primary" (click)="addSymptom()">Add</button>
          </div>
          <div class="mt-2">
            <span *ngFor="let symptom of symptoms; let i = index" class="badge bg-primary me-2 mb-2">
              {{symptom}}
              <button type="button" class="btn-close btn-close-white ms-2" (click)="removeSymptom(i)"></button>
            </span>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-4">
            <label class="form-label">Age</label>
            <input type="number" class="form-control" formControlName="age">
          </div>
          <div class="col-md-6 mb-4">
            <label class="form-label">Gender</label>
            <select class="form-select" formControlName="gender">
              <option value="">Select gender</option>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
        </div>

        <div class="mb-4">
          <label class="form-label">Additional Notes</label>
          <textarea class="form-control" rows="3" formControlName="notes"></textarea>
        </div>

        <button type="submit" class="btn btn-primary" [disabled]="!checkForm.valid || symptoms.length === 0">
          Check Symptoms
        </button>
      </form>

      <div *ngIf="prediction" class="mt-5 prediction-results">
        <h3 class="mb-4">Analysis Results</h3>
        
        <div class="result-card mb-4">
          <div class="d-flex justify-content-between align-items-center">
            <h4>{{prediction.prediction}}</h4>
            <span [class]="'badge ' + getUrgencyClass()">{{prediction.urgencyLevel}} Urgency</span>
          </div>
          <div class="confidence-bar">
            <div class="progress">
              <div 
                class="progress-bar" 
                [style.width.%]="prediction.confidence * 100"
                [attr.aria-valuenow]="prediction.confidence * 100"
              >
                {{(prediction.confidence * 100).toFixed(0)}}% Confidence
              </div>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <h5>Possible Conditions</h5>
            <ul class="list-group">
              <li *ngFor="let condition of prediction.possibleConditions" class="list-group-item">
                {{condition}}
              </li>
            </ul>
          </div>
          <div class="col-md-6">
            <h5>Recommended Actions</h5>
            <ul class="list-group">
              <li *ngFor="let action of prediction.recommendedActions" class="list-group-item">
                <i class="bi bi-check-circle-fill text-success me-2"></i>
                {{action}}
              </li>
            </ul>
          </div>
        </div>

        <div class="mt-4 additional-notes">
          <h5>Additional Notes</h5>
          <p>{{prediction.additionalNotes}}</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .symptom-form-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem;
      background: white;
      border-radius: 15px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.05);
    }

    .badge {
      padding: 0.5rem 1rem;
      font-size: 0.9rem;
    }

    .result-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 10px;
    }

    .confidence-bar {
      margin-top: 1rem;
    }

    .progress {
      height: 25px;
      border-radius: 15px;
    }

    .progress-bar {
      background: linear-gradient(45deg, #4a69bd, #6c5ce7);
      transition: width 0.5s ease;
    }

    .additional-notes {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 10px;
    }
  `]
})
export class SymptomFormComponent {
  checkForm: FormGroup;
  currentSymptom: string = '';
  symptoms: string[] = [];
  prediction: SymptomCheckPrediction | null = null;

  constructor(
    private fb: FormBuilder,
    private predictionService: PredictionService
  ) {
    this.checkForm = this.fb.group({
      age: ['', [Validators.min(0), Validators.max(120)]],
      gender: [''],
      notes: ['']
    });
  }

  addSymptom() {
    if (this.currentSymptom.trim()) {
      this.symptoms.push(this.currentSymptom.trim());
      this.currentSymptom = '';
    }
  }

  removeSymptom(index: number) {
    this.symptoms.splice(index, 1);
  }

  onSubmit() {
    if (this.checkForm.valid && this.symptoms.length > 0) {
      const data = {
        symptoms: this.symptoms,
        patientAge: this.checkForm.get('age')?.value,
        patientGender: this.checkForm.get('gender')?.value,
        additionalNotes: this.checkForm.get('notes')?.value
      };

      this.predictionService.createPrediction(data.symptoms).subscribe(
        (result) => {
          this.prediction = result;
        },
        (error) => {
          console.error('Error getting prediction:', error);
        }
      );
    }
  }

  getUrgencyClass(): string {
    if (!this.prediction) return '';
    
    switch (this.prediction.urgencyLevel.toLowerCase()) {
      case 'high':
        return 'bg-danger';
      case 'medium':
        return 'bg-warning';
      case 'low':
        return 'bg-success';
      default:
        return 'bg-secondary';
    }
  }
}