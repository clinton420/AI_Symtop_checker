import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PredictionService } from '../../services/prediction.service';
import { SymptomCheckPrediction } from '../../models/prediction.model';

@Component({
  selector: 'app-prediction-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="prediction-list-container">
      <h2 class="mb-4">Recent Predictions</h2>
      
      <div class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Symptoms</th>
              <th>Prediction</th>
              <th>Confidence</th>
              <th>Urgency</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let prediction of predictions">
              <td>{{prediction.timestampUtc | date:'short'}}</td>
              <td>
                <span *ngFor="let symptom of prediction.symptoms" class="badge bg-light text-dark me-1">
                  {{symptom}}
                </span>
              </td>
              <td>{{prediction.prediction}}</td>
              <td>
                <div class="progress" style="height: 20px;">
                  <div 
                    class="progress-bar" 
                    [style.width.%]="prediction.confidence * 100"
                    [attr.aria-valuenow]="prediction.confidence * 100"
                  >
                    {{(prediction.confidence * 100).toFixed(0)}}%
                  </div>
                </div>
              </td>
              <td>
                <span [class]="'badge ' + getUrgencyClass(prediction.urgencyLevel)">
                  {{prediction.urgencyLevel}}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-2" (click)="viewDetails(prediction)">
                  View
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .prediction-list-container {
      padding: 2rem;
      background: white;
      border-radius: 15px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.05);
    }

    .progress {
      width: 100px;
    }

    .progress-bar {
      background: linear-gradient(45deg, #4a69bd, #6c5ce7);
    }

    .table th {
      border-top: none;
      background: #f8f9fa;
    }
  `]
})
export class PredictionListComponent implements OnInit {
  predictions: SymptomCheckPrediction[] = [];

  constructor(private predictionService: PredictionService) {}

  ngOnInit() {
    this.loadPredictions();
  }

  loadPredictions() {
    this.predictionService.getPredictions().subscribe(
      (predictions) => {
        this.predictions = predictions;
      },
      (error) => {
        console.error('Error loading predictions:', error);
      }
    );
  }

  getUrgencyClass(urgency: string): string {
    switch (urgency.toLowerCase()) {
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

  viewDetails(prediction: SymptomCheckPrediction) {
    // Implement view details logic
  }
}