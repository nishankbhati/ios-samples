using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using System.Drawing;

namespace TransitionsDemo.InteractionControllers
{
	public class CEPinchInteractionController : CEBaseInteractionController
	{
		private float startScale;

		public override float CompletionSpeed {
			get {
				return 1 - PercentComplete;
			}
		}

		public override void WireToViewController (UIViewController viewController, CEInteractionOperation operation)
		{
			this.operation = operation;
			this.viewController = viewController;
			PrepareGestureRecognizerInView (viewController.View);
		}

		private void PrepareGestureRecognizerInView (UIView view)
		{
			gestureRecognizer = new UIPinchGestureRecognizer (HandlePinch);
			view.AddGestureRecognizer (gestureRecognizer);
		}

		public void HandlePinch (UIPinchGestureRecognizer pinchGestureRecognizer)
		{
			switch (gestureRecognizer.State) {
			case UIGestureRecognizerState.Began:
				TrackGestureBegan (pinchGestureRecognizer);
				break;
			case UIGestureRecognizerState.Changed:
				TrackGestureChaged (pinchGestureRecognizer);
				break;
			case UIGestureRecognizerState.Ended:
			case UIGestureRecognizerState.Cancelled:
				TrackGestureEnded (pinchGestureRecognizer);
				break;
			}
		}

		private void TrackGestureEnded (UIPinchGestureRecognizer pinchGestureRecognizer)
		{
			InteractionInProgress = false;
			if (!shouldCompleteTransition || pinchGestureRecognizer.State == UIGestureRecognizerState.Cancelled) {
				CancelInteractiveTransition ();
			} else {
				FinishInteractiveTransition ();
			}
		}

		private void TrackGestureChaged (UIPinchGestureRecognizer pinchGestureRecognizer)
		{
			// compute the current pinch fraction
			float fraction = 1f - pinchGestureRecognizer.Scale / startScale;
			shouldCompleteTransition = (fraction > 0.5);
			UpdateInteractiveTransition (fraction);
		}

		private void TrackGestureBegan (UIPinchGestureRecognizer pinchGestureRecognizer)
		{
			startScale = pinchGestureRecognizer.Scale;

			// start an interactive transition!
			InteractionInProgress = true;

			// perform the required operation
			if (operation == CEInteractionOperation.Pop) {
				viewController.NavigationController.PopViewControllerAnimated (true);
			} else {
				viewController.DismissViewController (true, null);
			}
		}
	}
}

