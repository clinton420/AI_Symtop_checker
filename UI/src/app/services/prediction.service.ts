import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { SymptomCheckPrediction } from '../models/prediction.model';

@Injectable({
  providedIn: 'root'
})
export class PredictionService {
  constructor(private api: ApiService) {}

  getPredictions(): Observable<SymptomCheckPrediction[]> {
    return this.api.get<SymptomCheckPrediction[]>('predictions');
  }

  getPrediction(id: string): Observable<SymptomCheckPrediction> {
    return this.api.get<SymptomCheckPrediction>(`predictions/${id}`);
  }

  createPrediction(symptoms: string[]): Observable<SymptomCheckPrediction> {
    return this.api.post<SymptomCheckPrediction>('predictions', { symptoms });
  }
}