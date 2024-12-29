public interface IDragParent
{
	void OnStartDrag(UIButton b);

	void OnDrag(UIButton b);

	void OnEndDrag(UIButton b, bool cancel = false);
}
