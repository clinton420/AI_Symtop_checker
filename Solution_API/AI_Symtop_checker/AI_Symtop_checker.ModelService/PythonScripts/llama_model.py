# PythonScripts/llama_model.py
import transformers
import torch
import datetime
import re

# Global variables
model_id = "ContactDoctor/Bio-Medical-Llama-3-8B"
pipeline = None
is_model_loaded = False

def load_model():
    """Load the model"""
    global pipeline, is_model_loaded
    
    try:
        print("Loading Bio-Medical-Llama-3-8B model... This may take several minutes.")
        
        # Create pipeline for text generation
        pipeline = transformers.pipeline(
            "text-generation",
            model=model_id,
            model_kwargs={"torch_dtype": torch.bfloat16},
            device_map="auto",
        )
        
        is_model_loaded = True
        print("Model loaded successfully!")
        return True
    except Exception as e:
        print(f"Error loading model: {str(e)}")
        return False

def process_llm_response(response_text, symptoms):
    """Process the LLM's text response and convert it to structured format"""
    # Default values in case parsing fails
    default_response = {
        "prediction": "Unknown",
        "confidence": 0.5,
        "possible_conditions": [],
        "urgency_level": "Medium",
        "recommended_actions": ["Consult a healthcare professional"],
        "additional_notes": "Unable to determine specific diagnosis from model output.",
        "timestamp": datetime.datetime.now().isoformat()
    }
    
    try:
        # Extract the primary diagnosis/prediction
        # Look for typical diagnostic language patterns
        prediction_patterns = [
            r"most likely (?:diagnosis|condition) (?:is|would be) ([^\.]+)",
            r"(?:likely|probably|possibly) (?:suffering from|having|experiencing) ([^\.]+)",
            r"(?:diagnosis|assessment) (?:points to|indicates|suggests) ([^\.]+)",
            r"(?:appears to be|presenting with symptoms of) ([^\.]+)",
            r"(?:the|your) (?:symptoms are consistent with|symptoms suggest) ([^\.]+)"
        ]
        
        prediction = "Unknown"
        for pattern in prediction_patterns:
            matches = re.search(pattern, response_text, re.IGNORECASE)
            if matches:
                prediction = matches.group(1).strip()
                break
                
        # If we couldn't find a clear diagnosis, use the first mentioned condition
        if prediction == "Unknown":
            common_conditions = [
                "Common Cold", "Flu", "COVID-19", "Seasonal Allergies", 
                "Sinus Infection", "Migraine", "Tension Headache", 
                "Bronchitis", "Pneumonia", "Strep Throat"
            ]
            
            for condition in common_conditions:
                if condition.lower() in response_text.lower():
                    prediction = condition
                    break
        
        # Extract possible conditions
        possible_conditions = [prediction]  # Start with the main prediction
        
        # Look for lists of conditions or differential diagnoses
        condition_pattern = r"(?:possibilities include|differential diagnosis|could also be|other possibilities|consider|such as) ([^\.]+)"
        condition_matches = re.search(condition_pattern, response_text, re.IGNORECASE)
        
        if condition_matches:
            conditions_text = condition_matches.group(1)
            # Split by common separators
            additional_conditions = [c.strip() for c in re.split(r',|\band\b|;|\bor\b', conditions_text)]
            possible_conditions.extend([c for c in additional_conditions if c and c != prediction])
        
        # Keep only unique conditions, limit to 3-5 for relevance
        possible_conditions = list(dict.fromkeys(possible_conditions))[:3]
        
        # Determine urgency level
        urgency_level = "Medium"  # Default
        if any(term in response_text.lower() for term in ["emergency", "immediate", "urgent", "severe", "call 911"]):
            urgency_level = "High"
        elif any(term in response_text.lower() for term in ["mild", "minor", "low risk", "self-care", "over-the-counter"]):
            urgency_level = "Low"
            
        # Extract recommended actions
        recommended_actions = []
        
        # Check for numbered/bulleted recommendations
        action_pattern = r"\d+\.\s+([^\d\n]+)"
        action_matches = re.findall(action_pattern, response_text)
        
        if action_matches:
            recommended_actions = [action.strip() for action in action_matches]
        else:
            # Look for phrases suggesting actions
            action_phrases = [
                r"(?:recommend|advised|suggested|should) ([^\.]+)",
                r"(?:important to|necessary to|need to) ([^\.]+)",
                r"(?:treatment|management) includes ([^\.]+)"
            ]
            
            for phrase in action_phrases:
                phrase_matches = re.findall(phrase, response_text, re.IGNORECASE)
                if phrase_matches:
                    recommended_actions.extend([match.strip() for match in phrase_matches])
                    
        # If we still don't have recommendations, add a generic one
        if not recommended_actions:
            recommended_actions = ["Consult a healthcare professional for proper diagnosis and treatment"]
        
        # Limit to 3-5 most relevant recommendations
        recommended_actions = recommended_actions[:3]
        
        # Set confidence based on language used
        confidence = 0.5  # Default medium confidence
        if any(term in response_text.lower() for term in ["most likely", "strongly suggests", "clear indication", "consistent with"]):
            confidence = 0.85
        elif any(term in response_text.lower() for term in ["possible", "might be", "consider", "uncertain", "unclear"]):
            confidence = 0.35
        
        # Additional notes - extract relevant contextual information
        additional_notes = "Based on the symptoms and context provided."
        notes_pattern = r"(?:note that|keep in mind|important to know|be aware) ([^\.]+)"
        notes_match = re.search(notes_pattern, response_text, re.IGNORECASE)
        
        if notes_match:
            additional_notes = notes_match.group(1).strip()
        
        # Format and return the structured response
        return {
            "prediction": prediction,
            "confidence": round(confidence, 2),
            "possible_conditions": possible_conditions,
            "urgency_level": urgency_level,
            "recommended_actions": recommended_actions,
            "additional_notes": additional_notes,
            "timestamp": datetime.datetime.now().isoformat()
        }
    
    except Exception as e:
        print(f"Error processing LLM response: {str(e)}")
        return default_response

def analyze_symptoms(symptoms, patient_age, patient_gender, additional_notes=""):
    """Analyze symptoms and provide structured results"""
    global pipeline, is_model_loaded
    
    # Check if model is loaded
    if not is_model_loaded:
        if not load_model():
            return {
                "error": "Failed to load model",
                "status": "error"
            }
    
    try:
        # Format the input
        symptoms_list = ", ".join(symptoms)
        formatted_input = f"""
        I'm a {patient_age}-year-old {patient_gender.lower()} experiencing the following symptoms:
        - {symptoms_list}
        
        Additional context: {additional_notes}
        
        What could be the possible diagnosis based on these symptoms? What should I do next?
        """
        
        # System prompt
        system_prompt = """
        You are an expert trained on healthcare and biomedical domain! 
        In your response, please:
        1. Clearly identify the most likely diagnosis
        2. List other possible conditions to consider
        3. Indicate the urgency level of the situation
        4. Provide specific recommended actions, numbered as a list
        5. Include any important notes about the condition
        """
        
        # Define conversation messages
        messages = [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": formatted_input},
        ]
        
        # Apply chat template
        prompt = pipeline.tokenizer.apply_chat_template(
            messages, 
            tokenize=False, 
            add_generation_prompt=True
        )
        
        # Define termination tokens
        terminators = [
            pipeline.tokenizer.eos_token_id, 
            pipeline.tokenizer.convert_tokens_to_ids("<|eot_id|>")
        ]
        
        # Generate response
        outputs = pipeline(
            prompt,
            max_new_tokens=512,
            eos_token_id=terminators,
            do_sample=True,
            temperature=0.6,
            top_p=0.9,
        )
        
        # Extract response
        raw_response = outputs[0]["generated_text"][len(prompt):]
        
        # Process response
        return process_llm_response(raw_response, symptoms)
        
    except Exception as e:
        print(f"Error analyzing symptoms: {str(e)}")
        return {
            "error": str(e),
            "status": "error"
        }