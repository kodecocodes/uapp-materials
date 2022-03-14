/*
 * Copyright (c) 2021 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderList : MonoBehaviour
{
    public OrderItem orderPrefab;

    private List<OrderItem> orders = new List<OrderItem>();

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void AddItem(Recipe recipe)
    {
        OrderItem newOrder = Instantiate(orderPrefab, transform);
        newOrder.Init(recipe);
        Vector2 spawnPos = new Vector2(rectTransform.rect.width, 0);// + (transform as RectTransform).sizeDelta.x
        Vector2 targetPos = rectTransform.anchorMin + new Vector2(120 * orders.Count, 0);
        newOrder.Lerp(spawnPos, targetPos);
        orders.Add(newOrder);
    }

    public void RemoveItem(Recipe recipe)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Recipe == recipe)
            {
                OrderItem toRemove = orders[i];
                toRemove.Lerp((toRemove.transform as RectTransform).anchoredPosition, new Vector2(-1000, 0));
                orders.RemoveAt(i);
                StartCoroutine(DelayedCleanUp(toRemove.gameObject, 1f));
                Shuffle();
                return;
            }
        }
    }

    private IEnumerator DelayedCleanUp(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(g);
    }

    private void Shuffle()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            Vector2 position = orders[i].rectTransform.anchoredPosition;
            Vector2 targetPos = rectTransform.anchorMin + new Vector2(120 * i, 0);
            orders[i].Lerp(position, targetPos);
        }
    }
}
