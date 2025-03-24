export interface SymptomCheckPrediction {
  id: string;
  symptoms: string[];
  prediction: string;
  confidence: number;
  timestampUtc: Date;
  possibleConditions: string[];
  urgencyLevel: string;
  recommendedActions: string[];
  additionalNotes: string;
}